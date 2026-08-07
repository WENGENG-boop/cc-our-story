// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

namespace OurStory.Services.Accounts;

/// <summary>
/// 口令散列。用 .NET 自带的 PBKDF2，不引入任何第三方依赖。
///
/// 存储格式：<c>pbkdf2-sha256$迭代次数$盐(Base64)$散列(Base64)</c>。迭代次数写在串里，以后调高了也能继续校验旧口令
/// </summary>
public static class PasswordHasher {
    private const string Prefix = "pbkdf2-sha256";
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int DefaultIterations = 210_000;

    /// <summary>
    /// 对用户密码进行哈希处理。
    /// 
    /// 使用 PBKDF2-SHA256 算法：
    /// - 生成随机盐值；
    /// - 通过多轮密钥派生增加破解成本；
    /// - 将算法参数、盐值和结果一起编码保存。
    /// 
    /// 返回结果可以直接存储到数据库中，
    /// 后续通过 <see cref="Verify"/> 方法进行验证。
    /// </summary>
    /// <param name="password">用户原始密码</param>
    /// <returns>
    /// 返回包含算法信息、迭代次数、盐值和哈希结果的存储字符串
    /// </returns>
    public static string Hash(string password) {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, DefaultIterations, HashAlgorithmName.SHA256, KeySize);

        return string.Join('$', Prefix, DefaultIterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    /// <summary>
    /// 校验用户输入密码是否匹配保存的哈希值
    /// 
    /// 校验流程：
    /// - 解析保存的算法参数；
    /// - 使用保存的盐值重新计算哈希；
    /// - 使用固定时间比较避免时间侧信道攻击。
    /// 
    /// 格式错误、参数异常或密码不匹配时，
    /// 均返回 false，不向调用方抛出异常
    /// </summary>
    /// <param name="password">用户输入的原始密码</param>
    /// <param name="hash">数据库中保存的密码哈希字符串</param>
    /// <returns>
    /// 密码匹配返回 true；
    /// 密码错误或哈希格式无效返回 false。
    /// </returns>
    public static bool Verify(string password, string? hash) {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash)) {
            return false;
        }

        var parts = hash.Split('$');
        if (parts.Length != 4 || parts[0] != Prefix || !int.TryParse(parts[1], out var iterations) || iterations <= 0) {
            return false;
        }

        byte[] salt;
        byte[] expected;
        try {
            salt = Convert.FromBase64String(parts[2]);
            expected = Convert.FromBase64String(parts[3]);
        } catch (FormatException) {
            return false;
        }

        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    /// <summary>
    /// 生成随机可读密码。
    /// 
    /// 用于首次部署、初始化管理员账号等场景。
    /// 
    /// 字符集合已移除容易混淆的字符：
    /// - 0 / O
    /// - 1 / l / I
    /// 
    /// 生成结果更适合人工查看和输入。
    /// </summary>
    /// <param name="length">密码长度</param>
    /// <returns>
    /// 返回指定长度的随机密码字符串。
    /// </returns>
    public static string GenerateReadablePassword(int length = 14) {
        // 去掉了容易看错的 0/O/1/l/I
        const string alphabet = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return RandomNumberGenerator.GetString(alphabet, length);
    }
}
